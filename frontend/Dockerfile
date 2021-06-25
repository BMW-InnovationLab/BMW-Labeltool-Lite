FROM node:lts as builder

LABEL maintainer="jon@doe.com"

WORKDIR /src/app
COPY ./code ./

RUN yarn install && yarn build:prod

FROM nginx:stable-alpine

LABEL maintainer="jon@doe.com"

COPY ./config/nginx.conf /etc/nginx/nginx.conf
COPY --from=builder /src/app/dist/labeltool/ /usr/share/nginx/html

EXPOSE 80
