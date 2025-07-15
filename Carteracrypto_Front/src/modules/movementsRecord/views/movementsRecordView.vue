<!-- eslint-disable vue/multi-word-component-names -->
<!----eslint-disable-next-line vue/multi-word-component-names-->

<template>
  <div>
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

    <div v-if="transactions.length > 0">
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
          <tr
            v-for="tx in transactions"
            :key="tx.id"
            :class="{ selected: selectedTransactionId === tx.id }"
            @click="selectTransaction(tx)"
          >
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
}

.selected {
  background-color: #d0f0ff !important;
}

table {
  border-collapse: collapse;
  width: 100%;
}

td,
th {
  padding: 8px;
  text-align: left;
}

tr:hover {
  background-color: #f0f0f0;
}

button {
  margin-right: 4px;
}
</style>
