variable "location" {
  description = "Azure region for all Ticketio resources"
  type        = string
  default     = "uksouth"
}

variable "project_name" {
  description = "Project name used in resource naming"
  type        = string
  default     = "ticketio"
}

variable "latest_auth_image_tag" {
  type        = string
  description = "Git commit SHA used to tag the Auth API container image"
}

variable "latest_api_image_tag" {
  type        = string
  description = "Git commit SHA used to tag the API proxy container image"
}

variable "jwt_key" {
  type        = string
  description = "Jwt key value for authorisation of internal apis"
  sensitive   = true
}

variable "jwt_issuer" {
  type        = string
  description = "Jwt issuer"
}

variable "jwt_audience" {
  type        = string
  description = "Jwt audience"
}

variable "auth_db_connection_string" {
  type        = string
  description = "Auth api SQL connection string for Ticketio database"
  sensitive   = true
}