import { createRouter, createWebHistory } from 'vue-router'


const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    /* {
      path: '/',
      name: 'home',
      component: HomeView,
    },*/
    {
      path: '/clientsRegister',
      name: 'clientsRegister',
      component: () => import('../modules/records/views/clientsRegisterView.vue'),
    },
     {
      path: '/newPurchase',
      name: 'newPurchase',
      component: () => import('../modules/newPurchase/components/NewPurchase.vue'),
    },
     {
      path: '/newSale',
      name: 'newSale',
      component: () => import('../modules/newSale/components/NewSale.vue'),
    },
     {
      path: '/movementsRecord',
      name: 'movementsRecord',
      component: () => import('../modules/movementsRecord/components/MovementsRecord.vue'),
    },
  ],
})

export default router
