// IndexedDB operations for Loan Cache Demo with version-based cache invalidation
window.loanCacheDB = {
    dbName: 'LoanDatabase',
    version: 2,
    loansStoreName: 'Loans',
    metadataStoreName: 'Metadata',
    
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
                
                // Create or recreate loans store
                if (db.objectStoreNames.contains(this.loansStoreName)) {
                    db.deleteObjectStore(this.loansStoreName);
                }
                const loansStore = db.createObjectStore(this.loansStoreName, { keyPath: 'id', autoIncrement: true });
                loansStore.createIndex('name', 'name', { unique: false });
                
                // Create metadata store for version information
                if (db.objectStoreNames.contains(this.metadataStoreName)) {
                    db.deleteObjectStore(this.metadataStoreName);
                }
                const metadataStore = db.createObjectStore(this.metadataStoreName, { keyPath: 'key' });
            };
        });
    },
    
    // Clear all loans and metadata
    clearLoans: async function() {
        const db = await this.initDB();
        return new Promise((resolve, reject) => {
            const transaction = db.transaction([this.loansStoreName, this.metadataStoreName], 'readwrite');
            const loansStore = transaction.objectStore(this.loansStoreName);
            const metadataStore = transaction.objectStore(this.metadataStoreName);
            
            const clearLoans = loansStore.clear();
            const clearMetadata = metadataStore.clear();
            
            transaction.oncomplete = () => {
                resolve();
            };
            
            transaction.onerror = () => {
                reject(new Error('Failed to clear data'));
            };
        });
    },
    
    // Store loan envelope (data + metadata)
    storeLoanEnvelope: async function(envelope) {
        const db = await this.initDB();
        return new Promise((resolve, reject) => {
            const transaction = db.transaction([this.loansStoreName, this.metadataStoreName], 'readwrite');
            const loansStore = transaction.objectStore(this.loansStoreName);
            const metadataStore = transaction.objectStore(this.metadataStoreName);
            
            // Clear existing data
            loansStore.clear();
            metadataStore.clear();
            
            // Store metadata
            metadataStore.add({
                key: 'version',
                value: envelope.version,
                timestamp: envelope.timestamp
            });
            
            // Store loans
            envelope.data.forEach(loan => {
                loansStore.add(loan);
            });
            
            transaction.oncomplete = () => {
                resolve();
            };
            
            transaction.onerror = () => {
                reject(new Error('Failed to store loan envelope'));
            };
        });
    },
    
    // Get all loans
    getAllLoans: async function() {
        const db = await this.initDB();
        return new Promise((resolve, reject) => {
            const transaction = db.transaction([this.loansStoreName], 'readonly');
            const store = transaction.objectStore(this.loansStoreName);
            const request = store.getAll();
            
            request.onsuccess = () => {
                resolve(request.result || []);
            };
            
            request.onerror = () => {
                reject(new Error('Failed to get all loans'));
            };
        });
    },
    
    // Get cached version
    getCachedVersion: async function() {
        const db = await this.initDB();
        return new Promise((resolve, reject) => {
            const transaction = db.transaction([this.metadataStoreName], 'readonly');
            const store = transaction.objectStore(this.metadataStoreName);
            const request = store.get('version');
            
            request.onsuccess = () => {
                resolve(request.result?.value || null);
            };
            
            request.onerror = () => {
                reject(new Error('Failed to get cached version'));
            };
        });
    },
    
    // Get cached metadata
    getCachedMetadata: async function() {
        const db = await this.initDB();
        return new Promise((resolve, reject) => {
            const transaction = db.transaction([this.metadataStoreName], 'readonly');
            const store = transaction.objectStore(this.metadataStoreName);
            const request = store.get('version');
            
            request.onsuccess = () => {
                resolve(request.result || null);
            };
            
            request.onerror = () => {
                reject(new Error('Failed to get cached metadata'));
            };
        });
    }
};