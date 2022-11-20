// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
    modules: ['nuxt-icons'],
    css: [
        'vuetify/lib/styles/main.sass',
        '@fortawesome/fontawesome-svg-core/styles.css'
    ],
    build: {
        transpile: ['vuetify'],
    }
})
