export default {
  // Global page headers: https://go.nuxtjs.dev/config-head
  head: {
    title: 'Anime Plex',
    htmlAttrs: {
      lang: 'en'
    },
    meta: [
      { charset: 'utf-8' },
      { name: 'viewport', content: 'width=device-width, initial-scale=1' },
      { hid: 'description', name: 'description', content: '' },
      { name: 'format-detection', content: 'telephone=no' }
    ],
    link: [
      { rel:"stylesheet", href:'https://cdn.jsdelivr.net/npm/bootstrap-icons@1.8.1/font/bootstrap-icons.css' },
      { rel:"stylesheet", href:'https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css', integrity:'sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3', crossorigin: 'anonymous'},
      { rel:"stylesheet", href:"https://vjs.zencdn.net/7.20.1/video-js.css" }
    ],
    script:[
      { src:"https://vjs.zencdn.net/7.20.1/video.min.js" }
    ]
  },

  // Global CSS: https://go.nuxtjs.dev/config-css
  css: [
  ],

  // Plugins to run before rendering page: https://go.nuxtjs.dev/config-plugins
  plugins: [
    '@/plugins/axios',
  ],

  // Auto import components: https://go.nuxtjs.dev/config-components
  components: true,

  // Modules for dev and build (recommended): https://go.nuxtjs.dev/config-modules
  buildModules: [
  ],

  // Modules: https://go.nuxtjs.dev/config-modules
  modules: [
    // https://go.nuxtjs.dev/axios
    '@nuxtjs/axios',
    'bootstrap-vue/nuxt'
  ],
  
  // Axios module configuration: https://go.nuxtjs.dev/config-axios
  axios: {
    // Workaround to avoid enforcing hard-coded localhost:3000: https://github.com/nuxt-community/axios-module/issues/308
    baseURL: '/',
  },

  // Build Configuration: https://go.nuxtjs.dev/config-build
  build: {
  },

  server:{
    host: "0.0.0.0"
  },

  publicRuntimeConfig: {
    ipAPI: process.env.HOST_API || "localhost",
    portAPI: process.env.PORT_API || "5000",
    protocolAPI: process.env.PROTOCOL_API || "http",

    ipWebSocket: process.env.HOST_WS || "ws://192.168.20.10:1234/room",
    basePath: process.env.BASE_PATH || "/",
    ipHttpServer: process.env.HOST_HTTP_SERVER || "localhost",
    portHttpServer: process.env.PORT_HTTP_SERVER || "8080"
  }
}
