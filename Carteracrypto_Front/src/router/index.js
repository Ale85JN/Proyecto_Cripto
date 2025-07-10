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
      path: '/newPurchaseView',
      name: 'newPurchase',
      component: () => import('../modules/newPurchase/views/newPurchaseView.vue'),
    },
     {
      path: '/newSaleView',
      name: 'newSale',
      component: () => import('../modules/newSale/views/newSaleView.vue'),
    },
     {
      path: '/movementsRecordView',
      name: 'movementsRecord',
      component: () => import('../modules/movementsRecord/views/movementsRecordView.vue'),
    },
  ],
})

export default router
