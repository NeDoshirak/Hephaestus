import { Profession, ProfessionDirection } from '@/types/vacancy';
import { Modal } from '@/components/Common/Modal';
import { ProfessionForm } from './ProfessionForm';

interface ProfessionDialogProps {
  isOpen: boolean;
  profession?: Profession;
  onClose: () => void;
  onSubmit: (data: { name: string; description?: string; direction: ProfessionDirection }) => void;
  isLoading?: boolean;
}

export function ProfessionDialog({
  isOpen,
  profession,
  onClose,
  onSubmit,
  isLoading,
}: ProfessionDialogProps) {
  return (
    <Modal
      title={profession ? 'Редактировать профессию' : 'Создать профессию'}
      isOpen={isOpen}
      onClose={onClose}
    >
      <ProfessionForm
        profession={profession}
        onSubmit={(data) => {
          onSubmit(data);
          onClose();
        }}
        isLoading={isLoading}
      />
    </Modal>
  );
}
