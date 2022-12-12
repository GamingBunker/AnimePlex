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
            socketBase: process.env.SOCKET_PATH_BASE_URL,
            httpBase: process.env.API_BASE_URL,
            basePath: process.env.BASE_PATH_BASE_URL,
            webBase: process.env.SHARE_ROOM_BASE_URL
        }
    }
})
