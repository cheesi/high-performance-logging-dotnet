<template>
  <footer
    v-if="$slidev.nav.currentLayout !== 'cover'"
    class="absolute bottom-0 left-0 right-0 p-2 text-xs"
  >
    📄 {{ $slidev.nav.currentPage }} / {{ $slidev.nav.total }} <carbon-barrier /> Licensed under CC BY 3.0
  </footer>
</template>