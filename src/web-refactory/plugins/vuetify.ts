//vuetify
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
// @ts-ignore
import colors from 'vuetify/lib/util/colors'
import { fa } from 'vuetify/iconsets/fa-svg'

//font awesome
import { library, config } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import { fas as fasFree } from '@fortawesome/free-solid-svg-icons'
config.autoAddCss = false
library.add(fasFree);

export default defineNuxtPlugin(nuxtApp => {
    const vuetify = createVuetify({
        components,
        directives,
        theme: {
            themes: {
                light: {
                    colors:{
                        primary: '#90cbd3',
                        secondary: '#202020',
                        info: colors.teal.lighten1,
                        warning: colors.amber.base,
                        error: '#F3565D',
                        success: '#5cb85c',
                    }
                },
            },
        },
        icons:{
            defaultSet: 'fa',
            aliases: {
                database:'fa-database',
                search:'fa-search',
                planet: 'fa-earth-europe',
                book: 'fa-book',
                x:'xmark',
                arrowLeft:'arrow-left',
                link: 'fa-link',
                download: 'fa-download',
                redownload: 'fa-rotate-left',
                trash: 'fa-trash-can',
                sortUp: 'fa-angle-up',
                sortDown: 'fa-angle-down',
                error: 'fa-circle-exclamation'
            },
            sets: {
                fa
            }
        }
    })
    nuxtApp.vueApp.component('font-awesome-icon', FontAwesomeIcon)
    nuxtApp.vueApp.use(vuetify)
})