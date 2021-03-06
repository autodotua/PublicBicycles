import Vue from 'vue'
import VueRouter, { RouteConfig } from 'vue-router'
import Home from '../views/Home.vue'
import Car from '../views/Car.vue'
import Recharge from '../views/Recharge.vue'
import Records from '../views/Records.vue'
import Database from '../views/Database.vue'
import Routes from '../views/Routes.vue'
import Admin from '../views/Admin.vue'
import Heatmap from '../views/Heatmap.vue'
import Password from '../views/Password.vue'
import Leaderboard from '../views/Leaderboard.vue'
import Move from '../views/Move.vue'

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
    path: '/admin',
    name: 'Admin',
    component: Admin
  },
  {
    path: '/heatmap',
    name: 'Heatmap',
    component: Heatmap
  },
  {
    path: '/password',
    name: 'Password',
    component: Password
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
    path: '/Records',
    name: 'Records',
    component: Records
  },
  {
    path: '/Move',
    name: 'Move',
    component: Move
  },
  {
    path: '/Leaderboard',
    name: 'Leaderboard',
    component: Leaderboard
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
