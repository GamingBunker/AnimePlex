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
            socketBase: process.env.SOCKET_PATH,
            httpBase: process.env.HTTP_PATH,
            basePath: process.env.BASE_PATH,
            webBase: process.env.SHARE_ROOM'
        }
    }
})
