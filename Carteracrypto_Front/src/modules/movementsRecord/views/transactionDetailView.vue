<template>
  <div v-if="transaction">
    <h2 v-if="mode === 'view'">Transaction Details</h2>
    <h2 v-else>Edit Transaction</h2>

    <form @submit.prevent="handleSubmit">
      <div class="form">
        <label>Cryptocurrency:</label>
        <input v-model="transaction.cryptoCode" :disabled="isViewMode" required />
      </div>

      <div class="form">
        <label>Action:</label>
        <select v-model="transaction.action" :disabled="isViewMode" required>
          <option value="purchase">Purchase</option>
          <option value="sale">Sale</option>
        </select>
      </div>

      <div class="form">
        <label>Amount:</label>
        <input type="number" step="0.00001" v-model="transaction.cryptoAmount" :disabled="isViewMode" required />
      </div>

      <div class="form">
        <label>Date and Time:</label>
        <input type="datetime-local" v-model="formattedDate" :disabled="isViewMode" required />
      </div>

      <div class="form">
        <label>Client ID:</label>
        <input type="number" v-model="transaction.clientId" :disabled="isViewMode" required />
      </div>

      <div v-if="!isViewMode" class="form">
        <button type="submit">Save Changes</button>
      </div>
    </form>

    <div class="form">
      <button @click="goBack">Back</button>
    </div>
  </div>

  <div v-else>
    <p>Loading transaction...</p>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';

const route = useRoute();
const router = useRouter();

const id = route.params.id;
const mode = route.query.mode || 'view';
const isViewMode = computed(() => mode === 'view');

const transaction = ref(null);
const formattedDate = ref('');

onMounted(async () => {
  const res = await fetch(`https://localhost:7189/api/CryptoTransaction/${id}`);
  const data = await res.json();
  transaction.value = data;
  
  const date = new Date(data.datetime);
  formattedDate.value = date.toISOString().slice(0, 16);
});

const handleSubmit = async () => {
  const payload = {
    cryptoCode: transaction.value.cryptoCode,
    action: transaction.value.action,
    cryptoAmount: transaction.value.cryptoAmount,
    datetime: formattedDate.value.replace('T', ' '),
    clientId: transaction.value.clientId
  };

  const res = await fetch(`https://localhost:7189/api/CryptoTransaction/${id}`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload)
  });

  if (res.ok) {
    alert("Transaction updated successfully!");
    router.push('/movementsRecord');
  } else {
    alert("Error updating transaction.");
  }
};

const goBack = () => {
  router.back();
};
</script>

<style scoped>
.form {
  margin-bottom: 12px;
}
input[disabled], select[disabled] {
  background-color: #f1f1f1;
}
</style>
