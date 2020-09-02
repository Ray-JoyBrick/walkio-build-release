terraform {
  required_providers {
    google = {
      source = "hashicorp/google"
    }
  }
}

# variable "private_key_path" {}
# variable "gcs_bucket_name" {}

provider "google" {
  version = "3.5.0"

  # credentials = file("${var.private_key_path}")
  credentials = file("/terraform/use-terraform.json")
  # credentials = "use-terraform.json"

  project = "walkio-271711"
  region  = "us-west1"
  # zone    = "us-central1-c"
}

resource "random_id" "instance_id" {
  byte_length = 8
}

# Need to enable API
resource "google_sourcerepo_repository" "ucb-build-release" {
  name = "ucb-build-release"
}

resource "google_sourcerepo_repository" "ucb-build-asset" {
  name = "ucb-build-asset"
}

resource "google_storage_bucket" "joybrick-walkio-dev" {
  name = "joybrick-walkio-dev"
  location = "us-west1"
  storage_class = "STANDARD"
}

# Need to enable API
resource "google_cloudbuild_trigger" "trigger-build-release" {
  name = "trigger-build-release"

  description = "Build release version"

  trigger_template {
    branch_name = "^prepare-build-release$"
    repo_name   = "ucb-build-release"
  }

  # tags = ["build"]

  substitutions = {
    _GCS_BUCKET = "joybrick-walkio-dev"
  }

  filename = "cloudbuild-build-release.yaml"
}

resource "google_cloudbuild_trigger" "trigger-build-asset" {
  name = "trigger-build-asset"

  description = "Build asset version"

  trigger_template {
    branch_name = "^prepare-build-asset$"
    repo_name   = "ucb-build-asset"
  }

  # tags = ["build"]

  substitutions = {
    _GCS_BUCKET = "joybrick-walkio-dev"
  }

  filename = "cloudbuild-build-asset.yaml"
}
