# 设置基础镜像
FROM nginx
WORKDIR /
COPY dist/  /usr/share/nginx/html/