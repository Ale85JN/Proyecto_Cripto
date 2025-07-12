<template>
  <div>
    <h2>Editar Cliente</h2>
    <form @submit.prevent="onSubmit">
      <div>
        <label for="name">Nombre:</label>
        <input id="name" v-model="client.name" type="text" required />
      </div>

      <div>
        <label for="email">Email:</label>
        <input id="email" v-model="client.email" type="email" required />
      </div>

      <div style="margin-top: 12px;">
        <button type="submit">Guardar Cambios</button>
        <button type="button" @click="goBack" style="margin-left: 8px;">Cancelar</button>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'

const router = useRouter()
const route = useRoute()

const client = ref({
  id: null,
  name: '',
  email: ''
})

const clientId = route.params.id

onMounted(async () => {
  try {
    const res = await fetch(`https://localhost:7189/api/Client/${clientId}`)
    if (!res.ok) throw new Error('Cliente no encontrado')
    const data = await res.json()
    client.value = data
  } catch (error) {
    alert('Error loading client')
    console.error(error)
    router.push('/clients')
  }
})

const onSubmit = async () => {
  try {
    const res = await fetch(`https://localhost:7189/api/Client/${clientId}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(client.value)
    })

    if (res.status === 204) {
      alert('Cliente actualizado correctamente')
      router.push('/clients')
    } else {
      alert('Error al actualizar cliente')
    }
  } catch (error) {
    alert('Error al actualizar cliente')
    console.error(error)
  }
}

const goBack = () => {
  router.push('/clients')
}
</script>

<style scoped>
div {
  margin-bottom: 8px;
}
label {
  display: inline-block;
  width: 70px;
}
input {
  padding: 6px;
  width: 250px;
}
button {
  padding: 6px 12px;
}
</style>
