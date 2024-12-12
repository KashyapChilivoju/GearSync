<script lang="ts">
    import { goto } from "$app/navigation";

    function navigateHome() {
        goto("/");
    }
</script>

<main>
    <header class="header" on:click={navigateHome} style="cursor: pointer;">
        <img src="/favicon.png" alt="GearSync Logo" class="logo" />
        <h1>GearSync</h1>
    </header>
</main>

<style>
    main {
        display: flex;
        flex-direction: column;
        align-items: center;
        font-family: "Arial", sans-serif;
        margin: 1rem;
        background-color: #041322;
    }

    .header {
        display: flex;
        align-items: center;
        gap: 0.5rem;
        width: 100%;
        justify-content: center;
        border-bottom: 2px solid #ddd;
        padding: 1rem 0;
        cursor: pointer;
    }

    .logo {
        width: 250px;
        height: auto;
    }

    h1 {
        font-size: 7rem;
        color: #D8DBDE;
        margin: 0;
    }
</style>
