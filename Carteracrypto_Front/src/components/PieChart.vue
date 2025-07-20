<template>
  <div style="max-width: 500px; margin-top: 24px;">
    <Pie :data="chartData" :options="chartOptions" />
  </div>
</template>

<!-- <script setup>
import { computed } from 'vue'
import { Pie } from 'vue-chartjs'
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  ArcElement
} from 'chart.js'

ChartJS.register(Title, Tooltip, Legend, ArcElement)

const props = defineProps({
  data: Array
})

const chartData = computed(() => {
  const portfolio = {}


  props.data
    .filter(t => t.action === 'purchase')
    .forEach(t => {
      const code = t.cryptoCode.toUpperCase()
      portfolio[code] = (portfolio[code] || 0) + t.cryptoAmount
    })

  const labels = Object.keys(portfolio)
  const values = Object.values(portfolio)

  return {
    labels,
    datasets: [
      {
        label: 'Crypto Holdings',
        data: values,
        backgroundColor: ['#f39c12', '#3498db', '#2ecc71', '#e74c3c', '#9b59b6']
      }
    ]
  }
})

const chartOptions = {
  responsive: true,
  plugins: {
    legend: {
      position: 'bottom'
    },
    title: {
      display: true,
      text: 'Crypto Portfolio Composition'
    }
  }
}
</script>
 -->
<script setup>
import { computed } from 'vue'
import { Pie } from 'vue-chartjs'
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  ArcElement
} from 'chart.js'
import ChartDataLabels from 'chartjs-plugin-datalabels' // <-- 👈 Importá el plugin

ChartJS.register(Title, Tooltip, Legend, ArcElement, ChartDataLabels) // <-- 👈 Registralo

const props = defineProps({
  data: Array
})

const chartData = computed(() => {
  const portfolio = {}

  props.data
    .filter(t => t.action === 'purchase')
    .forEach(t => {
      const code = t.cryptoCode.toUpperCase()
      portfolio[code] = (portfolio[code] || 0) + t.cryptoAmount
    })

  const labels = Object.keys(portfolio)
  const values = Object.values(portfolio)

  return {
    labels,
    datasets: [
      {
        label: 'Crypto Holdings',
        data: values,
        backgroundColor: ['#f39c12', '#3498db', '#2ecc71', '#e74c3c', '#9b59b6']
      }
    ]
  }
})

const chartOptions = {
  responsive: true,
  plugins: {
    legend: {
      position: 'bottom'
    },
    title: {
      display: true,
      text: 'Crypto Portfolio Composition'
    },
    datalabels: {
      color: '#fff',
      formatter: (value, context) => {
        const total = context.chart.data.datasets[0].data.reduce((a, b) => a + b, 0)
        const percentage = (value / total) * 100
        return percentage.toFixed(1) + '%'
      },
      font: {
        weight: 'bold',
        size: 14
      }
    }
  }
}
</script>
