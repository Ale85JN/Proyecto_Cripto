<template>
  <div>
    <h2>New Clients Registration Form</h2>
    <Form  :validation-schema="schema" @submit="onSubmit">
      <div class="form">
        <label for="name">Name:</label>
        <Field  v-model="name"  type="text" name="name" id="name" placeholder="Enter your Name"/>
        <ErrorMessage  name="name"></ErrorMessage>
      </div>
      <div  class="form">
        <label for=" email">Email:</label>
        <Field  v-model="email" type="email" name="email" id="email" placeholder="Enter your Email"/>
        <ErrorMessage  name="email"></ErrorMessage>
      </div>
      <div class="form" >
        <button type="submit">Register</button>
      </div>
    </Form>
  </div>

</template>

<script setup>
import {Form, Field, ErrorMessage} from 'vee-validate';
import {schema} from '../schemas/clientsValidationSchemas';
import { ref } from 'vue';

const name = ref('');
const email = ref('');

const onSubmit = async () => {
 const newClient = {
  name: name.value,
  email: email.value
 };
 console.log("==> Datos del cliente a enviar:", newClient);
 try {
 const response = await fetch('https://localhost:7189/api/Client',{
  method:'POST',
  headers: {'Content-Type':'application/json'},
  body: JSON.stringify(newClient)
 });

 console.log("==> Respuesta del servidor:", response);
 if(!response.ok) throw new Error('Error saving Client');

 alert('Client saved successfully');
 name.value = '';
 email.value = '';

 }
 catch(error){
    console.log("==> Respuesta del servidor:", error);
  alert('An Error occurred while saving the Client')
 }
};
</script>

<style scoped>
.form{
  margin-bottom: 10px;
}
</style>
