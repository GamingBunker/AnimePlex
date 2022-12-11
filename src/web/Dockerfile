FROM node:16.14

WORKDIR /app

COPY ["src/web/", "./"]
COPY ["src/web/.env_template", "./.env"]

COPY . .

RUN npm install
RUN npm run build

ENV NODE_TLS_REJECT_UNAUTHORIZED=0

ENV API_BASE_URL_BASE=''
ENV HTTP_PATH_BASE=''
ENV SOCKET_PATH_BASE=''
ENV SHARE_ROOM_BASE=''
ENV BASE_PATH_BASE=''

EXPOSE 3000

CMD [ "npm", "run", "preview"]
