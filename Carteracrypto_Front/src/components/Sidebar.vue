<!-- eslint-disable vue/multi-word-component-names -->
<!----eslint-disable-next-line vue/multi-word-component-names-->
<template>
  <div class="sidebar">
    <h2>Operations</h2>
    <ul>
      <li><router-link to="/clientsRegister">New Clients Register</router-link></li><br>
      <li><router-link to="/clients">Clients</router-link></li><br>
      <li><router-link to="/newPurchaseView">New Purchase</router-link></li><br>
      <li><router-link to="/portfolioChartView">Graphic Portfolio</router-link></li><br>
      <li><router-link to="/newSaleView">New Sale</router-link></li><br>
      <li> <router-link to="/movementsRecordView">Movements Record</router-link></li><br>
      <li> <router-link to="/">Home</router-link></li><br>

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
.sidebar a.router-link-active {
  font-weight: bold;
  color: darkblue;
}
</style>
