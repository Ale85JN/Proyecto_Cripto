<!-- eslint-disable vue/multi-word-component-names -->
<!----eslint-disable-next-line vue/multi-word-component-names-->
<template>
  <div class="sidebar">
    <h2>Operations</h2>
    <ul>
      <li><router-link to="/clientsRegister">New Clients Register</router-link></li>
      <li><router-link to="/newSaleView">New Sale</router-link></li>
      <li><router-link to="/newPurchaseView">New Purchase</router-link></li>
      <li> <router-link to="/movementsRecordView">Movements Record</router-link></li>
    </ul>
  </div>
</template>

<script setup>

</script>

<style scoped>
.sidebar{
width: 180px;
background-color:  lightgray;
padding: 15px;
}
</style>
