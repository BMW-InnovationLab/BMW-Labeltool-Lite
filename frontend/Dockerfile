FROM node:14.21.3 as builder

WORKDIR /src/app
COPY ./code ./

# RUN yarn config set proxy 
# RUN yarn config set https-proxy 


RUN yarn install && yarn build:prod

FROM nginx:stable-alpine

COPY ./config/nginx.conf /etc/nginx/nginx.conf
COPY --from=builder /src/app/dist/labeltool/ /usr/share/nginx/html

EXPOSE 80
