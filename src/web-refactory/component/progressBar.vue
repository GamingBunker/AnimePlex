<template>
  <v-progress-linear
      v-model="item.percentualDownload"
      :striped="item.percentualDownload < 100"
      :color="getStatus"
      height="25"
      striped
  >
    <template v-slot:default="{ value }">
      <template v-if="this.item.stateDownload.toUpperCase() === 'DOWNLOADING'">
        <strong>{{ this.item.percentualDownload }} %</strong>
      </template>
      <template v-else>
        <strong style="color: white">{{ this.item.stateDownload.toUpperCase() }}</strong>
      </template>
    </template>
  </v-progress-linear>
</template>

<script>
export default {
  name: "progressBar",
  props: [
    'item',
    'type'
  ],
  computed: {
    getStatus() {
      if (this.type === 'anime') {
        switch (this.item.stateDownload) {
          case 'downloading':
            return 'primary';
          case 'conversioning':
            return 'info';
          case 'completed':
            return 'success';
          case 'failed':
            return 'error';
          case 'wait conversion':
            return 'gray';
          case 'pending':
            return 'warning';
          default:
            return 'gray';
        }
      }
    }
  }
}
</script>