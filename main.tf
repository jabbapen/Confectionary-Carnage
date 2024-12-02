terraform {
  required_providers {
    aws = { source = "hashicorp/aws", version = "5.17.0" }
  }

  backend "s3" {
      bucket = "tfstate-confectionary-carnage"
      key = "app-state"
      region = "us-west-1"
  }

}

provider "aws" {
  region  = "us-west-1"
  profile = "default"
}

resource "aws_vpc" "main" {
  cidr_block = "10.0.0.0/16"

  tags = {
    Name = "main-vpc"
  }
}

resource "aws_subnet" "public" {
  vpc_id            = aws_vpc.main.id
  cidr_block        = "10.0.1.0/24"
  availability_zone = "us-west-1b"

  tags = {
    Name = "public-subnet"
  }
}

resource "aws_subnet" "private_az1" {
  vpc_id            = aws_vpc.main.id
  cidr_block        = "10.0.2.0/24"
  availability_zone = "us-west-1b"

  tags = {
    Name = "private-subnet"
  }
}

resource "aws_subnet" "private_az2" {
  vpc_id            = aws_vpc.main.id
  cidr_block        = "10.0.3.0/24"
  availability_zone = "us-west-1c"

  tags = {
    Name = "private-subnet-az2"
  }
}

resource "aws_internet_gateway" "igw" {
  vpc_id = aws_vpc.main.id

  tags = {
    Name = "main-internet-gateway"
  }
}

resource "aws_route_table" "public" {
  vpc_id = aws_vpc.main.id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.igw.id
  }

  tags = {
    Name = "public-route-table"
  }
}

resource "aws_route_table_association" "public" {
  subnet_id      = aws_subnet.public.id
  route_table_id = aws_route_table.public.id
}

resource "aws_security_group" "lambda_rds_sg" {
  vpc_id = aws_vpc.main.id

  ingress {
    from_port   = 5432
    to_port     = 5432
    protocol    = "tcp"
    cidr_blocks = ["10.0.0.0/16"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name = "lambda-rds-security-group"
  }
}

resource "aws_db_subnet_group" "rds" {
  name       = "rds-subnet-group"
  subnet_ids = [aws_subnet.private_az1.id, aws_subnet.private_az2.id]

  tags = {
    Name = "RDS subnet group"
  }
}

resource "aws_ecr_repository" "api" {
  name                 = "lambda-api"
  image_tag_mutability = "MUTABLE"
  force_delete         = true

  image_scanning_configuration {
    scan_on_push = true
  }
}

locals {
  repo_url = aws_ecr_repository.api.repository_url
}

resource "null_resource" "image" {
  triggers = {
    hash = md5(join("-", [for x in fileset("", ".backend/{*.py,*.txt,Dockerfile, *.toml}") : filemd5(x)]))
  }

  provisioner "local-exec" {
    command = <<EOF
      cd backend
      aws ecr get-login-password --region us-west-1 | docker login --username AWS --password-stdin ${local.repo_url}
      docker build --platform linux/amd64 -t ${local.repo_url}:latest .
      docker push ${local.repo_url}:latest
    EOF
  }
}

data "aws_ecr_image" "latest" {
  repository_name = aws_ecr_repository.api.name
  image_tag       = "latest"
  depends_on      = [null_resource.image]
}

resource "aws_iam_role" "lambda" {
  name = "lambda-execution-role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17",
    Statement = [
      {
        Effect    = "Allow",
        Principal = { Service = "lambda.amazonaws.com" },
        Action    = "sts:AssumeRole"
      }
    ]
  })
}

resource "aws_iam_policy" "lambda_vpc_access" {
  name = "Lambda-VPC-Access-Policy"

  policy = jsonencode({
    Version = "2012-10-17",
    Statement = [
      {
        Effect = "Allow",
        Action = [
          "ec2:CreateNetworkInterface",
          "ec2:DescribeNetworkInterfaces",
          "ec2:DeleteNetworkInterface"
        ],
        Resource = "*"
      }
    ]
  })
}

resource "aws_iam_role_policy_attachment" "lambda_basic_execution" {
  role       = aws_iam_role.lambda.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
}

resource "aws_iam_role_policy_attachment" "lambda_vpc_access" {
  role       = aws_iam_role.lambda.name
  policy_arn = aws_iam_policy.lambda_vpc_access.arn
}


resource "aws_iam_role_policy_attachment" "lambda_logs" {
  role       = aws_iam_role.lambda.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
} 

resource "aws_cloudwatch_log_group" "api" {
  name              = "/aws/lambda/api"
  retention_in_days = 14
}

resource "aws_lambda_function" "api" {
  function_name    = "api"
  role             = aws_iam_role.lambda.arn
  image_uri        = "${aws_ecr_repository.api.repository_url}@${data.aws_ecr_image.latest.image_digest}"
  package_type     = "Image"
  timeout          = 10

  environment {
    variables = {
      PG_HOST     = aws_db_instance.postgres.address
      PG_PORT     = var.pg_port
      PG_USER     = var.pg_user
      PG_PASSWORD = var.pg_password
      PG_DATABASE = var.pg_database
    }
  }

  vpc_config {
    subnet_ids         = [aws_subnet.private_az1.id, aws_subnet.private_az2.id]
    security_group_ids = [aws_security_group.lambda_rds_sg.id]
  }

  depends_on = [
    null_resource.image,
    aws_iam_role_policy_attachment.lambda_logs,
    aws_cloudwatch_log_group.api,
  ]
}

resource "aws_lambda_function_url" "api" {
  function_name      = aws_lambda_function.api.function_name
  authorization_type = "NONE"

  cors {
    allow_credentials = true
    allow_origins     = ["*"]
    allow_methods     = ["*"]
    allow_headers     = ["date", "keep-alive"]
    expose_headers    = ["keep-alive", "date"]
    max_age           = 86400
  }
}

resource "aws_db_instance" "postgres" {
  engine                 = "postgres"
  instance_class         = "db.t3.micro"
  allocated_storage      = 20
  db_name                = var.pg_database
  username               = var.pg_user
  password               = var.pg_password
  port                   = var.pg_port
  vpc_security_group_ids = [aws_security_group.lambda_rds_sg.id]
  db_subnet_group_name   = aws_db_subnet_group.rds.name
  skip_final_snapshot    = true

  tags = {
    Name = "Postgres Database"
  }
}


output "api_url" {
  value = aws_lambda_function_url.api.function_url
}

resource "aws_s3_bucket" "webgl_build" {
  bucket = "confectionary-carnage-webgl"
  force_delete = true
}


resource "aws_s3_bucket_website_configuration" "webgl_build" {
  bucket = aws_s3_bucket.webgl_build.id

  index_document {
    suffix = "index.html"
  }
}

resource "aws_s3_bucket_public_access_block" "webgl_build" {
  bucket = aws_s3_bucket.webgl_build.id

  block_public_acls       = false
  block_public_policy     = false
  ignore_public_acls      = false
  restrict_public_buckets = false
}

resource "aws_s3_bucket_policy" "webgl_build" {
  bucket = aws_s3_bucket.webgl_build.id
  depends_on = [aws_s3_bucket_public_access_block.webgl_build]

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Sid       = "PublicReadGetObject"
        Effect    = "Allow"
        Principal = "*"
        Action    = "s3:GetObject"
        Resource  = "${aws_s3_bucket.webgl_build.arn}/*"
      },
    ]
  })
}

resource "null_resource" "upload_webgl_build" {
  triggers = {
    build_hash = md5(join("-", [for f in fileset("./build", "**/*") : filemd5("./build/${f}")]))
  }

  provisioner "local-exec" {
    command = "aws s3 sync ./build s3://${aws_s3_bucket.webgl_build.bucket} --delete"
  }

  depends_on = [
    aws_s3_bucket.webgl_build,
    aws_s3_bucket_policy.webgl_build
  ]
}

output "website_url" {
  value = aws_s3_bucket_website_configuration.webgl_build.website_endpoint
}