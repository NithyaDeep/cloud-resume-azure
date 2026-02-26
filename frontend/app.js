const counterEl = document.getElementById("counter");

const API_BASE = "https://nithyacloudresfunc.azurewebsites.net/api";

async function getCount() {
  const r = await fetch(`${API_BASE}/counter`);
  if (!r.ok) throw new Error(`GET counter failed: ${r.status}`);
  return (await r.json()).count;
}

async function increment() {
  const r = await fetch(`${API_BASE}/counter/increment`, { method: "POST" });
  if (!r.ok) throw new Error(`POST increment failed: ${r.status}`);
  return (await r.json()).count;
}

(async () => {
  try {
    // Increment once per browser session
    const key = "crc_counted";
    let count;

    if (!sessionStorage.getItem(key)) {
      count = await increment();
      sessionStorage.setItem(key, "1");
    } else {
      count = await getCount();
    }

    counterEl.textContent = count;
  } catch (e) {
    console.error(e);
    counterEl.textContent = "—";
  }
})();