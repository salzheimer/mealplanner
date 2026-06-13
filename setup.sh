
#!/bin/bash

# check if deamon is running, if not start it
if (! docker stats --no-stream ); then
  echo "Starting Docker daemon..."
  open --background -a Docker
  # Wait for Docker to start
  while (! docker stats --no-stream ); do
    echo "Waiting for Docker to start..."
    sleep 1
  done
    echo "Docker daemon is running."
fi

# check if the volume exists, if not create it
if(! docker volume inspect identitydb_data >/dev/null 2>&1 ); then
  echo "Creating Docker volume 'identitydb_data'..."
  docker volume create identitydb_data
else
  echo "Docker volume 'identitydb_data' already exists."
fi
if(! docker volume inspect mealrecipedb_data >/dev/null 2>&1 ); then
  echo "Creating Docker volume 'mealrecipedb_data'..."
  docker volume create mealrecipedb_data
else
  echo "Docker volume 'mealrecipedb_data' already exists."
fi
if(! docker volume inspect plandb_data >/dev/null 2>&1 ); then
  echo "Creating Docker volume 'plandb_data'..."
  docker volume create plandb_data
else
  echo "Docker volume 'plandb_data' already exists."
fi

# start the containers
echo "Starting Docker containers..."
docker compose -f infrastructure/docker/docker-compose.yml up --build --force-recreate
