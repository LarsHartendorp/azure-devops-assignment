services:
  mongodb:
    image: mongo:latest
    container_name: mongo_container
    ports:
      - "27017:27017"  # Exposes MongoDB on localhost port 27017
    environment:
      MONGO_INITDB_ROOT_USERNAME: root
      MONGO_INITDB_ROOT_PASSWORD: password
    volumes:
      - mongo_data:/data/db  # Persistent storage for MongoDB data
  
  azurite:
    image: mcr.microsoft.com/azure-storage/azurite
    container_name: azurite
    ports:
      - "10000:10000" # Blob service
      - "10001:10001" # Queue service
      - "10002:10002" # Table service
    volumes:
      - azurite-data:/data  # Changed to match the volume name

volumes:
  mongo_data:
  azurite-data:  # Fixed to match the azurite service volume
