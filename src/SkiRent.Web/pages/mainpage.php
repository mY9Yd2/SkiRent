<script>
    document.addEventListener("DOMContentLoaded", function () {
        if (!sessionStorage.getItem("accessToken")) {
            window.location.href = "login.php";             // Ha nincs token, vissza a bejelentkezéshez
        }
    });
</script>