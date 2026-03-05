export const ConnectionStringTemplates: Record<string, string> = {
    'SQL Server': 'Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;TrustServerCertificate=True;',
    'MariaDB': 'Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;'
};

export const getTemplateForProvider = (provider: string): string => {
    return ConnectionStringTemplates[provider] || '';
};
