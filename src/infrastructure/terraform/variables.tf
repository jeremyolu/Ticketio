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