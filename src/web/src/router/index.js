import { createRouter, createWebHistory } from 'vue-router'

import animeView from "../views/animeView.vue"
import mangaView from "../views/mangaView.vue"
import resourceView from "../views/resourceView.vue"
import detailsView from "../views/detailsView.vue"
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    /*{
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/about',
      name: 'about',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../views/AboutView.vue')
    }*/
    {
      path: "/anime",
      name: "anime",
      component: animeView
    },
    {
      path: "/manga",
      name: "manga",
      component: mangaView
    },
    {
      path: "/resource",
      name: "resource",
      component: resourceView
    },
    {
      path: "/details",
      name: "details",
      component: detailsView
    }
  ]
})

export default router