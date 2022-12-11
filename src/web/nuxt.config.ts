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
            socketBase: process.env.NUXT_SOCKET_PATH_BASE,
            httpBase: process.env.NUXT_HTTP_PATH_BASE,
            basePath: process.env.NUXT_BASE_PATH_BASE,
            webBase: process.env.NUXT_SHARE_ROOM_BASE
        }
    }
})
