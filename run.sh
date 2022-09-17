rm Contracts/products.pb || true
protoc -I Contracts -I. --include_imports --include_source_info  --descriptor_set_out=Contracts/products.pb Contracts/products.proto
docker-compose up --build