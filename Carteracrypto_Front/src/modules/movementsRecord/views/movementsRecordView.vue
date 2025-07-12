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
          <tr v-for="tx in transactions" :key="tx.id">
            <td>{{ tx.cryptoCode.toUpperCase() }}</td>
            <td>{{ tx.action }}</td>
            <td>{{ tx.cryptoAmount }}</td>
            <td>{{ tx.money.toFixed(2) }}</td>
            <td>{{ new Date(tx.datetime).toLocaleString() }}</td>
            <td>{{ tx.clientName }}</td>
            <td>
              <button @click="viewTransaction(tx)">View</button>
              <button @click="editTransaction(tx)">Edit</button>
              <button @click="deleteTransaction(tx.id)">Delete</button>
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
import { ref, onMounted } from 'vue';

const clients = ref([]);
const transactions = ref([]);
const selectedClientId = ref('');

onMounted(async () => {
  try {
    const res = await fetch('https://localhost:7189/api/Client');
    clients.value = await res.json();
  } catch (error) {
    console.error('Error loading clients:', error);
  }
});

const fetchTransactions = async () => {
  if (!selectedClientId.value) return;

  try {
    const res = await fetch(`https://localhost:7189/api/CryptoTransaction/client/${selectedClientId.value}`);
    if (!res.ok) throw new Error('Failed to fetch transactions');
    transactions.value = await res.json();
  } catch (error) {
    console.error('Error loading transactions:', error);
  }
};

const viewTransaction = (tx) => {
  alert(`Viewing Transaction:\n\nCrypto: ${tx.cryptoCode}\nAmount: ${tx.cryptoAmount}\nDate: ${tx.datetime}`);
};

const editTransaction = async (tx) => {
  const newAmount = prompt('New crypto amount:', tx.cryptoAmount);
  if (!newAmount || isNaN(newAmount) || Number(newAmount) <= 0) return alert('Invalid amount.');

  try {
    const res = await fetch(`https://localhost:7189/api/CryptoTransaction/${tx.id}`, {
      method: 'PATCH',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ cryptoAmount: Number(newAmount) })
    });

    if (!res.ok) throw new Error('Failed to update');
    alert('Transaction updated!');
    await fetchTransactions();
  } catch (error) {
    console.error('Error editing transaction:', error);
  }
};

const deleteTransaction = async (id) => {
  if (!confirm('Are you sure you want to delete this transaction?')) return;

  try {
    const res = await fetch(`https://localhost:7189/api/CryptoTransaction/${id}`, {
      method: 'DELETE'
    });

    if (!res.ok) throw new Error('Failed to delete');
    alert('Transaction deleted!');
    await fetchTransactions();
  } catch (error) {
    console.error('Error deleting transaction:', error);
  }
};
</script>

<style scoped>
.form {
  margin-bottom: 15px;
}
table {
  width: 100%;
  border-collapse: collapse;
}
th,
td {
  padding: 8px;
  text-align: center;
}
</style>
