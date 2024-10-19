variable "pg_port" {
  description = "The PostgreSQL database port"
  default     = "5432"
}

variable "pg_user" {
  description = "The PostgreSQL database username"
}

variable "pg_password" {
  description = "The PostgreSQL database password"
  sensitive   = true
}

variable "pg_database" {
  description = "The PostgreSQL database name"
}

variable "pg_database" {
  description = "The PostgreSQL database name"
}
