FROM node:16.14

WORKDIR /app

COPY ["src/web/", "./"]

COPY . .

ENV NODE_TLS_REJECT_UNAUTHORIZED=0
ENV SOCKET_PATH_BASE_URL=''
ENV API_BASE_URL=''
ENV BASE_PATH_BASE_URL=''
ENV SHARE_ROOM_BASE_URL=''

RUN npm install
RUN npm run build

EXPOSE 3000

CMD [ "node", ".output/server/index.mjs"]
