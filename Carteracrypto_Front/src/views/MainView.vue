<template>

<div class="main-container">
    <Sidebar/>
    <div class="main-content">
      <div class="header">
        <h1>Crypto Wallet</h1>
      </div>

      <div class="content">
        <router-view />
        <div v-if="route.path === '/'" class="welcome-image">
         <img :src="cryptoBanner" alt="Crypto Banner" />
          <p>Welcome to the Crypto Portfolio</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import Sidebar from '../components/Sidebar.vue';
import { useRoute } from 'vue-router';
import cryptoBanner from '@/assets/crypto-banner.png';
const route = useRoute();
</script>

<style scoped>

.main-container {
  display: flex;
  height: 100vh;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  color: #3e2f23;
  background: #d1d1d1;
}

.sidebar {
  width: 180px;
  background-color:  #8fafa6;;
  padding: 15px;
}

.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.header {
  padding: 20px;
  text-align: center;
  background-color:  #aeafaf;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.content {
  flex: 1;
  padding: 20px;
  overflow-y: auto;
}

.welcome-image {
  text-align: center;
  margin-top: 30px;
}
.welcome-image img {
  max-width: 50%;
  height: auto;
  border-radius: 20px;
  box-shadow: 0 0 12px rgba(0, 0, 0, 0.2);
}
.welcome-image p {
  margin-top: 10px;
  font-size: 1.3rem;
  font-weight: bold;
  color: #333;
}
</style>
