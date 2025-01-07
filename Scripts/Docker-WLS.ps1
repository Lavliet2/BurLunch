wsl --install
wsl --list --verbose


#Показать список запущенных контейнеров:
docker ps

#Показать список всех контейнеров (включая остановленные):
docker ps -a

#Остановить контейнер:
docker stop <container_id>

#Удалить контейнер:
docker rm <container_id>

#Удалить все остановленные контейнеры:
docker container prune

#Просмотреть образы Docker (сохранённые контейнеры):
docker images

#Удалить образ:
docker rmi <image_id>




docker-compose down
docker-compose up --build


ASPNETCORE_ENVIRONMENT=Development docker-compose up -d


#Ubumtu
ssh lav-server

curl -L --output /usr/local/bin/gitlab-runner https://gitlab-runner-downloads.s3.amazonaws.com/latest/binaries/gitlab-runner-linux-amd64
chmod +x /usr/local/bin/gitlab-runner





Kuber token
eyJhbGciOiJSUzI1NiIsImtpZCI6Ijlpa1JpTlFsQzVhREhBVkxHNHdpc0RvRWtVU3dveWtILUVIeFl4enRBdUUifQ.eyJpc3MiOiJrdWJlcm5ldGVzL3NlcnZpY2VhY2NvdW50Iiwia3ViZXJuZXRlcy5pby9zZXJ2aWNlYWNjb3VudC9uYW1lc3BhY2UiOiJrdWJlLXN5c3RlbSIsImt1YmVybmV0ZXMuaW8vc2VydmljZWFjY291bnQvc2VjcmV0Lm5hbWUiOiJtaWNyb2s4cy1kYXNoYm9hcmQtdG9rZW4iLCJrdWJlcm5ldGVzLmlvL3NlcnZpY2VhY2NvdW50L3NlcnZpY2UtYWNjb3VudC5uYW1lIjoiZGVmYXVsdCIsImt1YmVybmV0ZXMuaW8vc2VydmljZWFjY291bnQvc2VydmljZS1hY2NvdW50LnVpZCI6ImJkZGNkNmVjLWY5MzctNGZjYy1iZDRlLTk2YTViYmYwZGRlNiIsInN1YiI6InN5c3RlbTpzZXJ2aWNlYWNjb3VudDprdWJlLXN5c3RlbTpkZWZhdWx0In0.oZ91s53R7kY0QZ5IcBPT93gabxUCP2wT8RVAgHrJuoNu-1gfNLo5egtIpmDb_bFp8tVYkNqDbbbOlQ1ByPF2TnkvAW9gxKL4atP3Y1I2AOjuvh4hBefMQY8GrwMQlECcb6mOTlzqkMum1zzGWNS2iqjHC_E-XjCUpTc7BqFcv0IT4J23S_71TEj45yGOeNm3joni8845IA_ovmOTrMgEo9f_-5P0zzcTR4UthRCVseeA9SasjHkmGjcPuQ-WbvSvJgLYb-_Trl_Rh6Wu-jjWOgWsP5OzZ4jgXBiEpWYlD8FvhSDsqvCI8zw9gcvTI6lSh-z-Lwg3mVUs-bOHxQC0_w