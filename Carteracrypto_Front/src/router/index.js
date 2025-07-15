import { createRouter, createWebHistory } from 'vue-router'


const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [


    {
      path: '/clientsRegister',
      name: 'clientsRegister',
      component: () => import('../modules/clients/views/clientsRegisterView.vue'),
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
    {
      path: '/transactions/:id',
      name: 'transactionDetail',
      component: () => import('../modules/movementsRecord/views/transactionDetailView.vue')
    },
   {
      path: '/clients',
      name: 'clientsList',
      component: () => import('../modules/clients/views/clientListView.vue')
    },
   {
      path: '/clients/:id',
      name: 'clientDetail',
      component: () => import('../modules/clients/views/clientDetailView.vue')
    },
   {
      path: '/portfolioChartView',
      name: 'portfolioChartView',
      component: () => import('../modules/portfolio/views/portfolioChartView.vue')
    }
  ],
})

export default router
