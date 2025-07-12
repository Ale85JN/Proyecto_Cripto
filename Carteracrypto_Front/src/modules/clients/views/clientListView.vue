<template>
  <div>
    <h2>Clients List</h2>
    <table>
      <thead>
        <tr>
          <th>Name</th>
          <th>Email</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="client in clients" :key="client.id">
          <td>{{ client.name }}</td>
          <td>{{ client.email }}</td>
          <td>
            <button @click="editClient(client.id)">Editar</button>
            <button @click="deleteClient(client.id)">Eliminar</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const clients = ref([])
const router = useRouter()


onMounted(async () => {
  try {
    const res = await fetch('https://localhost:7189/api/Client')
    if (!res.ok) throw new Error('Error loading clients')
    clients.value = await res.json()
  } catch (error) {
    alert('Error loading clients')
    console.error(error)
  }
})


const editClient = (id) => {
  router.push(`/clients/${id}/edit`)
}


const deleteClient = async (id) => {
  if (!confirm('¿Sure you want to delete this client?')) return

  try {
    const res = await fetch(`https://localhost:7189/api/Client/${id}`, {
      method: 'DELETE'
    })
    if (res.status === 204) {
      clients.value = clients.value.filter(c => c.id !== id)
      alert('Successfully deleted client')
    } else {
      alert('Error deleting client')
    }
  } catch (error) {
    alert('Error deleting client')
    console.error(error)
  }
}
</script>

<style scoped>
table {
  border-collapse: collapse;
  width: 100%;
}
th, td {
  border: 1px solid #ccc;
  padding: 8px;
}
button {
  margin-right: 5px;
}
</style>
