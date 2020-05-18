import Vue from 'vue'
import VueRouter, { RouteConfig } from 'vue-router'
import Home from '../views/Home.vue'
import Car from '../views/Car.vue'
import Recharge from '../views/Recharge.vue'
import Records from '../views/Records.vue'
import Database from '../views/Database.vue'
import Routes from '../views/Routes.vue'

Vue.use(VueRouter)

  const routes: Array<RouteConfig> = [
  {
    path: '/home',
    name: 'Home',
    component: Home
  },
  {
    path: '/database',
    name: 'Database',
    component: Database
  },
  {
    path: '/routes',
    name: 'Routes',
    component: Routes
  },
  {
    path: '/',
    name: 'Home',
    component: Home
  },
  {
    path: '/car',
    name: 'Car',
    component: Car
  },
  {
    path: '/recharge',
    name: 'Recharge',
    component: Recharge
  },
  {
    path: '/Records',
    name: 'Records',
    component: Records
  },
  {
    path: '/about',
    name: 'About',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import(/* webpackChunkName: "about" */ '../views/About.vue')
  },
  {
    path: '/login',
    name: 'Login',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import(/* webpackChunkName: "about" */ '../views/Login.vue')
  }
]

const router = new VueRouter({
  mode: 'hash',
  base: process.env.BASE_URL,
  routes
})

export default router
