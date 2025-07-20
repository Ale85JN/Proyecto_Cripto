<template>
<div>
  <h2>New Crypto Purchase</h2>
  <Form :validation-schema="schema" @submit="onSubmit">
    <div class="form">
      <label for="cryptoCode">Cryptocurrency:</label>
      <Field as="select" v-model="cryptoCode" name="cryptoCode">
        <option value="">Select</option>
        <option value="BTC">Bitcoin (BTC)</option>
        <option value="ETH">Ethereum (ETH)</option>
        <option value="USDT">Tether (USDT)</option>
      </Field>
      <ErrorMessage name="cryptoCode" />
    </div>

    <div class="form">
      <label for="cryptoAmount">Amount:</label>
      <Field v-model="cryptoAmount" type="number" step="0.00001" name="cryptoAmount"/>
       <ErrorMessage name="cryptoAmount" />
    </div>

    <div class="form">
      <label for="clientId">Client:</label>
      <Field as="select" v-model="clientId" name="clientId">
        <option value="">Select a client</option>
        <option v-for="client in clients" :key="client.id" :value="client.id">
          {{ client.name }} ({{ client.email }})
        </option>
      </Field>
      <ErrorMessage name="clientId" />
    </div>

    <div class="form">
      <label for="datetime">Date and Time:</label>
      <Field v-model="datetime" type="datetime-local" name="datetime"/>
      <ErrorMessage name="datetime" />
    </div>

    <div class="form">
      <button type="submit">Register Purchase</button>
    </div>
  </Form>
</div>
</template>

<script setup>
import {Form, Field, ErrorMessage } from 'vee-validate';
import {ref, onMounted} from 'vue';
import{schema} from '../schemas/purchaseValidationSchemas.js';

const cryptoCode =ref('');
const cryptoAmount =ref('');
const clientId =ref('');
const datetime =ref('');
const clients =ref([]);


console.log("Lista de clientes disponibles:", clients.value);
console.log("Cliente seleccionado:", clientId.value);

onMounted(async () => {
  try{

  const res = await fetch('https://localhost:7189/api/Client');
  const data = await res.json();
  clients.value = data;
  console.log("Clientes cargados:", clients.value);
}catch(error) {
  console.error("Error fetching clients:", error);
}
});

const onSubmit = async () => {
  const formattedDateTime = datetime.value.replace('T', ' ');

  const purchase = {
    cryptoCode: cryptoCode.value,
    action: 'purchase',
    cryptoAmount: parseFloat(cryptoAmount.value),
    clientId: parseInt(clientId.value),
    datetime: formattedDateTime
  };
  if (!clients.value.some(c => c.id === purchase.clientId)) {
  console.error("El clientId no coincide con ninguno en la base de datos.");
  return;
  }
  console.log("==> Payload a enviar:", purchase);
  try {
    const response = await fetch('https://localhost:7189/api/CryptoTransaction', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(purchase)
    });

    if (!response.ok) {throw new Error('Error saving Purchase');
    }

    alert('Purchase registered successfully!!');
    cryptoCode.value = '';
    cryptoAmount.value = '';
    clientId.value = '';
    datetime.value = '';
    window.location.href = '/';
  } catch (error) {
    console.error("==> Error al guardar la compra:", error);
    alert('Error saving purchase');
  }
};
</script>

<style scoped>
.form {
  margin-bottom: 10px;
}
</style>
