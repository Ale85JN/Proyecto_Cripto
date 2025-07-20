<!-- eslint-disable vue/multi-word-component-names -->
<!----eslint-disable-next-line vue/multi-word-component-names-->
<template>
<div>
  <h2>New Crypto Sale</h2>
  <Form :validation-schema="schema" @submit="onSubmit">
    <div class="form">
      <label for="cryptoCode">Cryptocurrency:</label>
      <Field as="select" name="cryptoCode" v-model="cryptoCode">
        <option disabled value="">Select a cryptocurrency</option>
        <option value="BTC">Bitcoin (BTC)</option>
        <option value="ETH">Ethereum (ETH)</option>
        <option value="USDT">Tether (USDT)</option>
      </Field>
      <ErrorMessage name="cryptoCode" />
    </div>

    <div class="form">
      <label for="criptoAmount">Amount:</label>
      <Field type="number" step="0.00001" name="cryptoAmount" v-model="cryptoAmount" />
      <ErrorMessage name="cryptoAmount" />
    </div>

    <div class="form">
      <label for="clientId">Client:</label>
     <Field as="select" name="clientId" v-model="clientId">
        <option value="">Select a client</option>
        <option v-for="client in clients" :key="client.id" :value="client.id">
          {{ client.name }} - {{ client.email }}
        </option>
     </Field>
     <ErrorMessage name="clientId" />
    </div>

    <div class="form">
    <label for="datetime">Date and Time:</label>
    <Field type="datetime-local" name="datetime" v-model="datetime"/>
    <ErrorMessage name="datetime" />
    </div>

    <div class="form">
      <button type="submit">Save Sale</button>
    </div>
  </Form>
</div>
</template>

<script setup>
import {ref, onMounted} from 'vue';
import {Form, Field, ErrorMessage} from 'vee-validate';
import { saleSchema as schema } from '../schemas/saleValidationSchemas.js';

const cryptoCode = ref('');
const cryptoAmount = ref(null);
const clientId = ref('');
const datetime = ref('');
const clients = ref([]);

onMounted(async () => {
  const res = await fetch('https://localhost:7189/api/Client');
  clients.value = await res.json();
});

const onSubmit = async () => {
  const formattedDateTime = datetime.value.replace('T', ' ');

  const sale = {
  cryptoCode: cryptoCode.value,
  cryptoAmount: parseFloat(cryptoAmount.value),
  clientId: parseInt(clientId.value),
  datetime: formattedDateTime,
  action: 'sale'
 };
  console.log("==> Objeto enviado al backend:", JSON.stringify(sale));
  console.log("==> Payload correcto a enviar:", sale);
  try {
    const res = await fetch ('https://localhost:7189/api/CryptoTransaction',{
      method: 'POST',
      headers: {'Content-Type': 'application/json'},
      body: JSON.stringify(sale)
    });

    console.log("==> Respuesta del servidor:", res);

    if(!res.ok) throw new Error('Failed to save transaction');

    alert('Sale saved Successfully!!');
    cryptoCode.value = '';
    cryptoAmount.value = null;
    clientId.value = '';
    datetime.value = '';
    window.location.href = '/movementsRecord';
  }
  catch (error) {
    console.error("==> Error al guardar la venta:", error);
    alert('An error occurred while saving the sale.');
  }
};


</script>

<style scoped>
.form {
  margin-bottom: 12px;
}
</style>

