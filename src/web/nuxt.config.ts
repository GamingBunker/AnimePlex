// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
    modules: [
        'nuxt-icons',
        '@pinia/nuxt'
    ],
    css: [
        'vuetify/lib/styles/main.sass',
        '@fortawesome/fontawesome-svg-core/styles.css',
        '~/assets/css/index.scss'
    ],
    build: {
        transpile: ['vuetify'],
    },
    runtimeConfig:{
        public: {
            socketBase: process.env.SOCKET_PATH || 'ws://localhost:1234/room',
            httpBase: process.env.HTTP_PATH || 'http://localhost:5002',
            basePath: process.env.BASE_PATH || '/public',
            webBase: process.env.SHARE_ROOM || 'http://localhost:3000'
        }
    }
})
