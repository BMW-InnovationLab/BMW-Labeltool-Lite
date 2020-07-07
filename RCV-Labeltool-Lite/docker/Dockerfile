FROM node:11 as builder

# Notes:
# - node 11 as apperently they used that to build the original project looking at the pinnned node-sass version
WORKDIR /src/app
RUN ls
COPY package.json yarn.lock ./
RUN yarn
COPY . ./
RUN yarn build:prod


FROM nginx

COPY docker/nginx.conf /etc/nginx/nginx.conf
COPY --from=builder /src/app/dist/labeltool/ /usr/share/nginx/html

EXPOSE 80
