<!-- eslint-disable vue/multi-word-component-names -->
<!----eslint-disable-next-line vue/multi-word-component-names-->

<template>

  <div class="container">
    <h2>Movements Record</h2>

    <div class="form">
      <label for="clientSelect">Select Client:</label>
      <select v-model="selectedClientId">
        <option value="">-- Choose a client --</option>
        <option v-for="client in clients" :key="client.id" :value="client.id">
          {{ client.name }} ({{ client.email }})
        </option>
      </select>
      <button @click="fetchTransactions">Search</button>
    </div>

    <div v-if="transactions.length > 0" class="table-wrapper">
      <table border="1">
        <thead>
          <tr>
            <th>Crypto</th>
            <th>Action</th>
            <th>Amount</th>
            <th>Money (ARS)</th>
            <th>Date</th>
            <th>Client</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="tx in transactions" :key="tx.id" :class="{ selected: selectedTransactionId === tx.id }" @click="selectTransaction(tx)">
            <td>{{ tx.cryptoCode.toUpperCase() }}</td>
            <td>{{ tx.action }}</td>
            <td>{{ tx.cryptoAmount }}</td>
            <td>{{ tx.money.toFixed(2) }}</td>
            <td>{{ new Date(tx.datetime).toLocaleString() }}</td>
            <td>{{ tx.clientName }}</td>
            <td>
              <button @click.stop="viewTransaction(tx)">View</button>
              <button @click.stop="editTransaction(tx)">Edit</button>
              <button @click.stop="deleteTransaction(tx.id)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-else-if="selectedClientId">
      <p>No transactions found for this client.</p>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const clients = ref([])
const transactions = ref([])
const selectedClientId = ref('')
const selectedTransactionId = ref(null)
const router = useRouter()

onMounted(async () => {
  try {
    const res = await fetch('https://localhost:7189/api/Client')
    clients.value = await res.json()
  } catch (error) {
    console.error('Error loading clients:', error)
  }
})

const fetchTransactions = async () => {
  if (!selectedClientId.value) return

  try {
    const res = await fetch(`https://localhost:7189/api/CryptoTransaction/client/${selectedClientId.value}`)
    if (!res.ok) throw new Error('Failed to fetch transactions')
    transactions.value = await res.json()
  } catch (error) {
    console.error('Error loading transactions:', error)
  }
}

const selectTransaction = (tx) => {
  selectedTransactionId.value = tx.id
}

const viewTransaction = (tx) => {
  selectTransaction(tx)
  router.push({ name: 'transactionDetail', params: { id: tx.id }, query: { mode: 'view' } })
}

const editTransaction = (tx) => {
  selectTransaction(tx)
  router.push({ name: 'transactionDetail', params: { id: tx.id }, query: { mode: 'edit' } })
}

const deleteTransaction = async (id) => {
  if (!confirm('Are you sure you want to delete this transaction?')) return

  try {
    const res = await fetch(`https://localhost:7189/api/CryptoTransaction/${id}`, {
      method: 'DELETE'
    })

    if (!res.ok) throw new Error('Failed to delete')
    await fetchTransactions()
    selectedTransactionId.value = null
  } catch (error) {
    console.error('Error deleting transaction:', error)
  }
}
</script>

<style scoped>
.form {
  margin-bottom: 12px;
  padding: 12px;
  background-color: #ffffff;
  border: 1px solid #ccc;
  border-radius: 6px;
  max-width: 500px;
}

.select{
padding: 8px;
  margin-right: 10px;
  font-size: 14px;
  border: 1px solid #999;
  border-radius: 4px;
  background-color:#f8f8f8;
}
.button {
  padding: 8px;
  margin-right: 10px;
  font-size: 14px;
  border: 1px solid #999;
  border-radius: 4px;
  background-color:#3498db;
}

button:hover {
  background-color:#2980b9;
  cursor: pointer;
}

table {
  width: 100%;
  border-collapse: collapse;
  background-color: #f9f9f9;
}

th, td {
  border: 1px solid #333;
  padding: 10px;
  text-align: left;
}

th {
  background-color: #ddd;
  color: #333;
}

tr:hover {
  background-color: #f5f5f5;
}

.selected {
  background-color: #cce5ff !important;
}
</style>
