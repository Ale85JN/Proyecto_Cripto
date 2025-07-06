<!-- eslint-disable vue/multi-word-component-names -->
<!----eslint-disable-next-line vue/multi-word-component-names-->
<template>
  <div class="sidebar">
    <h2>Operations</h2>
    <ul>
      <li><router-link to="/clientsRegister">New Clients Register</router-link></li>
      <li><router-link to="/newSale">New Sale</router-link></li>
      <li><router-link to="/newPurchase">New Purchase</router-link></li>
      <li> <router-link to="/movementsRecord">Movements Record</router-link></li>
    </ul>
  </div>
</template>

<script setup>

</script>

<style scoped>
.sidebar{
width: 200px;
background-color: grey;
padding: 20px;
}
</style>
