FROM node:22-alpine AS build

WORKDIR /app

COPY src/frontend/Ticketio.UI/package*.json ./

RUN npm ci

COPY src/frontend/Ticketio.UI/ ./

RUN npm run build

FROM nginx:alpine

COPY --from=build /app/dist /usr/share/nginx/html

COPY src/frontend/Ticketio.UI/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]