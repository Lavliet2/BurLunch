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