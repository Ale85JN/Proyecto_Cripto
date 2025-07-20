<template>
  <div>
    <h2>Cryptocurrency Portfolio Chart</h2>

    <div class="form">
      <label for="client">Select client:</label>
      <select v-model="selectedClientId" id="client">
        <option disabled value="">-- Select a Client --</option>
        <option v-for="client in clients" :key="client.id" :value="client.id">
          {{ client.name }} ({{ client.email }})
        </option>
      </select>
      <button @click="fetchTransactions">View Chart</button>
    </div>

    <PieChart v-if="transactions.length > 0" :data="transactions" />
    <p v-else-if="selectedClientId">This customer has no purchase transactions recorded.</p>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import PieChart from '@/components/PieChart.vue'

const clients = ref([])
const selectedClientId = ref('')
const transactions = ref([])

onMounted(async () => {
  try {
    const res = await fetch('https://localhost:7189/api/Client')
    clients.value = await res.json()
  } catch (err) {
    console.error('Error al cargar clientes', err)
  }
})

const fetchTransactions = async () => {
  if (!selectedClientId.value) return
  try {
    const res = await fetch(`https://localhost:7189/api/CryptoTransaction/client/${selectedClientId.value}`)
    const data = await res.json()
    transactions.value = data
  } catch (err) {
    console.error('Error al cargar transacciones', err)
  }
}
</script>

<style scoped>
.form {
  margin: 20px 0;
}
select {
  margin: 0 8px;
  padding: 4px;
}
button {
  padding: 6px 12px;
}
</style>
