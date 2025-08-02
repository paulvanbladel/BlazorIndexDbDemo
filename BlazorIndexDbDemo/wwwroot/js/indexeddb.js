// IndexedDB operations for Loan Cache Demo
window.loanCacheDB = {
    dbName: 'LoanDatabase',
    version: 1,
    storeName: 'Loans',
    
    // Initialize the database
    initDB: function() {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open(this.dbName, this.version);
            
            request.onerror = () => {
                reject(new Error('Failed to open database'));
            };
            
            request.onsuccess = (event) => {
                resolve(event.target.result);
            };
            
            request.onupgradeneeded = (event) => {
                const db = event.target.result;
                if (!db.objectStoreNames.contains(this.storeName)) {
                    const store = db.createObjectStore(this.storeName, { keyPath: 'id', autoIncrement: true });
                    store.createIndex('name', 'name', { unique: false });
                }
            };
        });
    },
    
    // Clear all loans
    clearLoans: async function() {
        const db = await this.initDB();
        return new Promise((resolve, reject) => {
            const transaction = db.transaction([this.storeName], 'readwrite');
            const store = transaction.objectStore(this.storeName);
            const request = store.clear();
            
            request.onsuccess = () => {
                resolve();
            };
            
            request.onerror = () => {
                reject(new Error('Failed to clear loans'));
            };
        });
    },
    
    // Add multiple loans
    addLoans: async function(loans) {
        const db = await this.initDB();
        return new Promise((resolve, reject) => {
            const transaction = db.transaction([this.storeName], 'readwrite');
            const store = transaction.objectStore(this.storeName);
            let completed = 0;
            
            transaction.oncomplete = () => {
                resolve();
            };
            
            transaction.onerror = () => {
                reject(new Error('Failed to add loans'));
            };
            
            loans.forEach(loan => {
                store.add(loan);
            });
        });
    },
    
    // Get all loans
    getAllLoans: async function() {
        const db = await this.initDB();
        return new Promise((resolve, reject) => {
            const transaction = db.transaction([this.storeName], 'readonly');
            const store = transaction.objectStore(this.storeName);
            const request = store.getAll();
            
            request.onsuccess = () => {
                resolve(request.result);
            };
            
            request.onerror = () => {
                reject(new Error('Failed to get loans'));
            };
        });
    }
};