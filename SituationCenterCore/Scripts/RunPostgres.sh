#!/bin/bash

docker run --name scbs-postgres -e POSTGRES_PASSWORD=password -p 5433:5432 -d postgres