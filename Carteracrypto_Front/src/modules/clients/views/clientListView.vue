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
            <button @click="editClient(client.id)">Edit</button>
            <button @click="deleteClient(client.id)">Delete</button>
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
  }
  catch (error) {
    alert('Error loading clients')
    console.error(error)
  }
})


const editClient = (id) => {
  router.push({ name: 'clientDetail', params: { id } })
}


const deleteClient = async (id) => {
  if (!confirm('Sure you want to delete this client?')) return

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
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

th, td {
  border: 2px solid #555;
  padding: 10px;
  text-align: left;
}

th {
  background-color: #f2f2f2;
  font-weight: bold;
  color: #333;
}

td {
  background-color: #fff;
  color: #222;
}

tr:hover {
  background-color: #f9f9f9;
}

button {
  margin-right: 5px;
  padding: 5px 10px;
  border: none;
  background-color: #3498db;
  color: white;
  cursor: pointer;
  border-radius: 4px;
}

button:hover {
  background-color: #2980b9;
}
.selected {
  background-color: #cce5ff !important;
}
</style>
