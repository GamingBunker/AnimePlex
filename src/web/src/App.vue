<template>
<div>
  <!-- Problem img tag src 403-->
  <meta name="referrer" content="no-referrer" />
  
  <!--<div style="position:absolute; margin: 10px;">
    <b-button v-b-toggle.sidebar-no-header style="background-color: #98ccd4; border:#98ccd4;"><i class="bi bi-caret-right"></i></b-button>
  </div>
  <b-sidebar id="sidebar-no-header" no-header shadow>
    <template #default="{ hide }">
      <div style="margin: 20px;">
         Space Disk 
        <div>
          <SpaceDiskComponent />
        </div>
         Health Services
        <div>
          <HealthServicesComponent />
        </div>
        <b-button variant="primary" block @click="hide">Close</b-button>
      </div>
    </template>
  </b-sidebar>-->

  <div class="container">
    <!-- head -->
    <div>
      <!-- title -->
      <div style="padding-bottom: 10px;">
        <img src="./assets/img/bar-anime.jpg" style="object-fit: contain; height: 220px; width: 100%;">
      </div>
    </div>
    <RouterView />
  </div>
</div>
</template>

<style>
body{
  background-image: url("./assets/img/background.jpg");
  background-size: contain;
  background-attachment: fixed;
}
</style>